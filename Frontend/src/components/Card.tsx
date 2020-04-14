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
}

export default (props: CardProps) =>  {
  const [isHovering, setisHovering] = useState(false);
  const { children, className, lr, hoverEffect, width, height, ...rest } = props;
  return (
    <Card
      className={`${className ?? ''}${hoverEffect ? ` card-${hoverEffect}` : ''}${
        lr ? ' lr' : ''
        }${isHovering ? ' temp': ' active'}`}
      onMouseOver={()=> setisHovering(true)}
      onMouseOut={()=> setisHovering(false)}
      css={css`
        max-width: ${width};
        max-height: ${height};
      `}
      {...rest}
    >
      {children}
    </Card>
  );
}
