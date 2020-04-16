/** @jsx jsx */
import React, { useState, useEffect, useRef } from 'react';
import { Card } from 'reactstrap';
import { css, jsx } from '@emotion/core';
import './Card.scoped.scss';
export interface CardProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  hoverEffect?: 'bubba' | undefined | null | '';
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
  lr?: boolean;
  width?: string;
  height?: string;
  colorA?: string;
  colorB?: string;
}

export default (props: CardProps) => {
  const [isHovering, setisHovering] = useState(false);
  const { children, className, lr, hoverEffect, width, height, ...rest } = props;

  let ref: any = useRef(null);

  useEffect(() => {
    let x = 0,
      y = 0,
      pX = 0,
      pY = 0,
      status = 0,
      timer = 0,
      sensitivity = 7,
      interval = 700,
      timeout = 9;
    let internalRef = ref.current;

    const delay = (e: React.MouseEvent<HTMLElement>) => {
      if (timer) {
        clearTimeout(timer);
      }
      status = 0;
      setisHovering(false);
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
        setisHovering(true);
      } else {
        pX = x;
        pY = y;
        timer = window.setTimeout(() => compare(e), interval);
      }
    };
    const dispatchOver = (e: React.MouseEvent<HTMLElement>) => {
      if (timer) {
        clearTimeout(timer);
      }
      if (internalRef) {
        internalRef.removeEventListener('mousemove', tracker, false);
      }

      if (status !== 1) {
        pX = e.clientX;
        pY = e.clientY;
        if (internalRef) {
          internalRef.addEventListener('mousemove', tracker, false);
        }
        timer = window.setTimeout(() => compare(e), interval);
      }
    };
    const dispatchOut = (e: React.MouseEvent<HTMLElement>) => {
      if (timer) {
        clearTimeout(timer);
      }
      if (internalRef) {
        internalRef.removeEventListener('mousemove', tracker, false);
      }

      if (status === 1) {
        timer = window.setTimeout(() => delay(e), timeout);
      }
    };
    // subscribe event
    if (internalRef) {
      internalRef.addEventListener('mouseover', dispatchOver, false);
      internalRef.addEventListener('mouseout', dispatchOut, false);
    }

    return () => {
      // unsubscribe event
      if (internalRef) {
        internalRef.removeEventListener('mouseover', dispatchOver, false);
        internalRef.removeEventListener('mouseout', dispatchOut, false);
      }
    };
  }, [ref]);

  return (
    <Card
      className={`${className ?? ''}${hoverEffect ? ` card-${hoverEffect}` : ''}${lr ? ' lr' : ''}${
        isHovering ? ' temp' : ' active'
      }`}
      css={css`
        max-width: ${width};
        max-height: ${height};
      `}
      ref={ref}
      {...rest}
    >
      {children}
    </Card>
  );
};
