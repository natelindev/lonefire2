/** @jsx jsx */
import React, { useState } from 'react';
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
  href?: string;
}

export default React.forwardRef((props: CardProps, ref: React.Ref<Card>) => {
  const { children, className, lr, hoverEffect, width, height, href, ...rest } = props;
  const [isEntering, setIsEntering] = useState(false);
  const [isLeaving, setIsLeaving] = useState(false);
  return (
    <Card
      className={`${className ?? ''}${hoverEffect ? ` card-${hoverEffect}` : ''}${lr ? ' lr' : ''}${
        isLeaving ? ' active' : ''
      }${isEntering ? ' temp' : ''}`}
      css={css`
        max-width: ${width};
        max-height: ${height};
      `}
      {...rest}
      innerRef={ref}
    >
      {children}
      {href ? (
        <a
          className="full-div-link z-1"
          href={href}
          css={css`
            font-size: 0;
          `}
        >
          href
        </a>
      ) : null}
    </Card>
  );
});
