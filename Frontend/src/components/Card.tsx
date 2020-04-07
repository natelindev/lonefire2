import * as React from 'react';
import { Card } from 'reactstrap';
import './Card.scoped.scss';

export interface CardProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  hoverEffect?: 'bubba' | undefined | null | '';
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
  lr?: boolean;
  colorA?: string;
  colorB?: string;
}

export default (props: CardProps) =>  {
  const { children, className, lr, hoverEffect, ...rest } = props;
  return (
    <Card
      className={`${className ?? ''}${hoverEffect ? ` card-${hoverEffect}` : ''}${
        lr ? ' lr' : ''
        }`}
      {...rest}
    >
      {children}
    </Card>
  );
}
