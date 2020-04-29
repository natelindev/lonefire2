/** @jsx jsx */
import React from 'react';
import { Card } from 'reactstrap';
import { css, jsx } from '@emotion/core';
import { useHoverIntent } from '../hooks/hoverIntent';

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

export default React.forwardRef((props: CardProps, ref: React.Ref<HTMLElement | null>) => {
  const { children, className, lr, hoverEffect, width, height, href, ...rest } = props;
  const [isHovering, intentRef] = useHoverIntent(ref);

  return (
    <Card
      className={`${className ?? ''}${hoverEffect ? ` card-${hoverEffect}` : ''}${lr ? ' lr' : ''}${
        isHovering ? ' temp' : ' active'
      }`}
      css={css`
        max-width: ${width};
        max-height: ${height};
      `}
      {...rest}
      innerRef={intentRef}
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
