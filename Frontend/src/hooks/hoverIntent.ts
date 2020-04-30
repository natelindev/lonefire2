import { useState, useEffect, useRef, useImperativeHandle } from 'react';

export function useHoverIntent(
  ref: React.Ref<HTMLElement | null>,
  options: { sensitivity: number; interval: number; timeout: number } = {
    sensitivity: 6,
    interval: 100,
    timeout: 0,
  }
) {
  const intentRef = useRef<HTMLElement>(null);
  const [isHovering, setIsHovering] = useState(false);
  useImperativeHandle(ref, () => intentRef.current, [intentRef]);

  let x = 0,
    y = 0,
    pX = 0,
    pY = 0,
    timer = 0;
  const delay = (e: MouseEvent) => {
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
    if (Math.sqrt((pX - x) * (pX - x) + (pY - y) * (pY - y)) < options.sensitivity) {
      return setIsHovering(true);
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
    if (intentRef.current) {
      intentRef.current.removeEventListener('mousemove', tracker, false);
    }
    if (!isHovering) {
      pX = e.clientX;
      pY = e.clientY;
      if (intentRef.current) {
        intentRef.current.addEventListener('mousemove', tracker, false);
      }
      timer = window.setTimeout(() => compare(e), options.interval);
    }
  };
  const dispatchOut = (e: MouseEvent) => {
    if (timer) {
      clearTimeout(timer);
    }
    if (intentRef.current) {
      intentRef.current.removeEventListener('mousemove', tracker, false);
    }
    if (isHovering) {
      timer = window.setTimeout(() => delay(e), options.timeout);
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

  return [isHovering, intentRef];
}
