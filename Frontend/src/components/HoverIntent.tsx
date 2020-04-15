import React, { useEffect } from 'react';

export interface HoverIntentProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  children: React.ReactElement;
  sensitivity: number;
  interval: number;
  timeout: number;
  onMouseOut: (event: React.MouseEvent<HTMLElement>) => void;
  onMouseOver: (event: React.MouseEvent<HTMLElement>) => void;
}

export default (props: HoverIntentProps) => {
  let { children, sensitivity, interval, timeout, onMouseOut, onMouseOver } = props;
  let x = 0,
    y = 0,
    pX = 0,
    pY = 0,
    status = 0,
    timer: NodeJS.Timeout = new NodeJS.Timeout();
  let element: any = null;

  useEffect(() => {
    React.cloneElement(children, {
      ref: (e: any) => {
        element = e;
      }
    });
    // subscribe event
    element.addEventListener('mouseover', dispatchOver, false);
    element.addEventListener('mouseout', dispatchOut, false);
    return () => {
      // unsubscribe event
      element.removeEventListener('mouseover', dispatchOver, false);
      element.removeEventListener('mouseout', dispatchOut, false);
    };
  }, []);

  const delay = (e: React.MouseEvent<HTMLElement>) => {
    if (timer) {
      clearTimeout(timer);
    }
    status = 0;
    return onMouseOut.call(element, e);
  };
  const tracker = (e: MouseEvent) => {
    x = e.clientX;
    y = e.clientY;
  };
  const compare = (e: React.MouseEvent<HTMLElement>) => {
    if (timer) {
      clearTimeout(timer);
    }
    if (Math.abs(pX - x) + Math.abs(pY - y) < sensitivity) {
      status = 1;
      return onMouseOver.call(element, e);
    } else {
      pX = x;
      pY = y;
      timer = setTimeout(() => compare(e), interval);
    }
  };
  const dispatchOver = (e: React.MouseEvent<HTMLElement>) => {
    if (timer) {
      clearTimeout(timer);
    }
    element.removeEventListener('mousemove', tracker, false);
    if (status !== 1) {
      pX = e.clientX;
      pY = e.clientY;
      element.addEventListener('mousemove', tracker, false);
      timer = setTimeout(() => compare(e), props.interval);
    }
  };
  const dispatchOut = (e: React.MouseEvent<HTMLElement>) => {
    if (timer) {
      clearTimeout(timer);
    }
    element.removeEventListener('mousemove', tracker, false);
    if (status === 1) {
      timer = setTimeout(() => delay(e), timeout);
    }
  };
  return element;
};
