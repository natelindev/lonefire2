import React, { useEffect, useRef, useImperativeHandle } from 'react';
import hoistNonReactStatic from 'hoist-non-react-statics';

export interface HoverIntentProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  customMouseOver: (e: MouseEvent) => {};
  customMouseLeave: (e: MouseEvent) => {};
  ref: React.Ref<HTMLElement>;
  innerRef?: React.Ref<HTMLElement>;
}

const HoverIntent = (
  Component: React.ComponentType,
  options: { sensitivity: number; interval: number; timeout: number } = {
    sensitivity: 7,
    interval: 100,
    timeout: 0,
  }
) => {
  const WrappedComponent = (props: HoverIntentProps, ref: React.Ref<any>) => {
    const element = useRef<HTMLElement>(null);
    let x = 0,
      y = 0,
      pX = 0,
      pY = 0,
      status = 0,
      timer = 0;
    const delay = (e: MouseEvent) => {
      if (timer) {
        clearTimeout(timer);
      }
      status = 0;
      return props.onEnter.call(element, e);
    };
    const tracker = (e: MouseEvent) => {
      x = e.clientX;
      y = e.clientY;
    };
    const compare = (e: MouseEvent) => {
      if (timer) {
        clearTimeout(timer);
      }
      if (Math.abs(pX - x) + Math.abs(pY - y) < options.sensitivity) {
        status = 1;
        return props.onLeave.call(element, e);
      } else {
        pX = x;
        pY = y;
        timer = window.setTimeout(() => compare(e), options.interval);
      }
    };
    const dispatchOver = (e: MouseEvent) => {
      if (timer) {
        clearTimeout(timer);
      }
      if (element.current) {
        element.current.removeEventListener('mousemove', tracker, false);
      }

      if (status !== 1) {
        pX = e.clientX;
        pY = e.clientY;
        if (element.current) {
          element.current.addEventListener('mousemove', tracker, false);
        }
        timer = window.setTimeout(() => compare(e), options.interval);
      }
    };
    const dispatchOut = (e: MouseEvent) => {
      if (timer) {
        clearTimeout(timer);
      }
      if (element.current) {
        element.current.removeEventListener('mousemove', tracker, false);
      }
      if (status === 1) {
        timer = window.setTimeout(() => delay(e), options.timeout);
      }
    };

    useEffect(() => {
      const currentElement = element.current;
      if (currentElement) {
        currentElement.addEventListener('mouseover', dispatchOver, false);
        currentElement.addEventListener('mouseout', dispatchOut, false);
      }

      return () => {
        if (currentElement) {
          currentElement.removeEventListener('mouseover', dispatchOver, false);
          currentElement.removeEventListener('mouseout', dispatchOut, false);
        }
      };
    });

    useImperativeHandle(
      ref,
      () => {
        return element.current;
      },
      [element]
    );
    // detect reactstrap components
    // if (!Component.prototype.render) {
    //   return <Component innerRef={element} {...props} />;
    // } else {
    return <Component ref={element} {...props} />;
    // }
  };
  hoistNonReactStatic(WrappedComponent, Component);
  return React.forwardRef(WrappedComponent);
};

export default HoverIntent;
