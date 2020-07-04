import { useState, useEffect, useRef, useImperativeHandle, RefObject } from 'react';

interface OptionType {
  ref?: React.Ref<HTMLElement | null>;
  sensitivity?: number;
  interval?: number;
  timeout?: number;
}

export function useHoverIntent(options: OptionType): [boolean, RefObject<HTMLElement>] {
  const { ref, sensitivity = 6, interval = 100, timeout = 0 } = options;
  const intentRef = useRef<HTMLElement>(null);
  const [isHovering, setIsHovering] = useState(false);

  let x = 0;
  let y = 0;
  let pX = 0;
  let pY = 0;
  let timer = 0;
  const delay = () => {
    if (timer) {
      clearTimeout(timer);
    }
    return setIsHovering(false);
  };
  const tracker = (e: MouseEvent) => {
    x = e.clientX;
    y = e.clientY;
  };
  const compare = (e: MouseEvent) => {
    if (timer) {
      clearTimeout(timer);
    }
    if (Math.abs(pX - x) + Math.abs(pY - y) < sensitivity) {
      return setIsHovering(true);
    }
    pX = x;
    pY = y;
    timer = window.setTimeout(() => compare(e), interval);
    return undefined;
  };
  const dispatchOver = (e: MouseEvent) => {
    if (timer) {
      clearTimeout(timer);
    }
    if (intentRef.current) {
      intentRef.current.removeEventListener('mousemove', tracker, false);
    }
    if (!isHovering) {
      pX = e.clientX;
      pY = e.clientY;
      if (intentRef.current) {
        intentRef.current.addEventListener('mousemove', tracker, false);
      }
      timer = window.setTimeout(() => compare(e), interval);
    }
  };
  const dispatchOut = () => {
    if (timer) {
      clearTimeout(timer);
    }
    if (intentRef.current) {
      intentRef.current.removeEventListener('mousemove', tracker, false);
    }
    if (isHovering) {
      timer = window.setTimeout(() => delay(), timeout);
    }
  };

  useEffect(() => {
    const currentRef = intentRef.current;
    if (currentRef) {
      currentRef.addEventListener('mouseover', dispatchOver, false);
      currentRef.addEventListener('mouseout', dispatchOut, false);
    }

    return () => {
      if (currentRef) {
        currentRef.removeEventListener('mouseover', dispatchOver, false);
        currentRef.removeEventListener('mouseout', dispatchOut, false);
      }
    };
  });

  useImperativeHandle(ref, () => intentRef.current, [intentRef]);

  return [isHovering, intentRef];
}
