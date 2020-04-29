import { useState, useEffect } from 'react';

export function useHoverIntent(element: React.RefObject<HTMLElement | null>, options: { sensitivity: number; interval: number; timeout: number } = {
    sensitivity: 6,
    interval: 100,
    timeout: 0,
  }) {
  const [isHovering, setisHovering] = useState(false);

  let x = 0,
      y = 0,
      pX = 0,
      pY = 0,
      timer = 0;
    const delay = (e: MouseEvent) => {
      if (timer) {
        clearTimeout(timer);
      }
      return setisHovering(false);
    };
    const tracker = (e: MouseEvent) => {
      x = e.clientX;
      y = e.clientY;
    };
    const compare = (e: MouseEvent) => {
      if (timer) {
        clearTimeout(timer);
      }
      if ( Math.sqrt((pX - x)*(pX - x) + (pY - y)*(pY - y)) < options.sensitivity) {
        return setisHovering(true);
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
      if (!isHovering) {
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
      if (isHovering) {
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

  return isHovering;
}