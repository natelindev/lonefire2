import React, { useEffect, useRef } from 'react';

export interface HoverIntentProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  children?: React.ReactElement;
  x: number;
  y: number;
  pX: number;
  pY: number;
  status: number;
  timer: NodeJS.Timeout;
}

export default (props: HoverIntentProps) => {
  let { x, y, pX, pY, status, timer, children, ...rest } = props;
  let element: React.ReactNode = null;
  useEffect(() => {
    element.addEventListener('mouseover', dispatchOver, false);
    element.addEventListener('mouseout', dispatchOut, false);

    return function cleanup() {
      element.removeEventListener('mouseover', dispatchOver, false);
      element.removeEventListener('mouseout', dispatchOut, false);
    };
  });

  const delay = (e: MouseEvent) => {
    if (timer) {
      clearTimeout(timer);
    }
    status = 0;
    return props.onMouseOut.call(element, e);
  };
  const tracker = (e: MouseEvent) => {
    x = e.clientX;
    y = e.clientY;
  };
  const compare = (e: MouseEvent) => {
    if (timer) {
      clearTimeout(timer);
    }
    if (Math.abs(pX - x) + Math.abs(pY - y) < props.sensitivity) {
      status = 1;
      return props.onMouseOver.call(element, e);
    } else {
      pX = x;
      pY = y;
      timer = setTimeout(() => compare(e), props.interval);
    }
  };
  const dispatchOver = (e: MouseEvent) => {
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
  const dispatchOut = (e: MouseEvent) => {
    if (timer) {
      clearTimeout(timer);
    }
    element.removeEventListener('mousemove', tracker, false);
    if (status === 1) {
      timer = setTimeout(() => delay(e), props.timeout);
    }
  };
  return React.cloneElement(children, {
    ref: (e: React.ReactNode) => {
      element = e;
    }
  });
};
