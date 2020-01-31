import * as React from 'react';
import { Card } from 'reactstrap';
import './Card.scoped.scss';

export interface CardProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  hoverEffect?: boolean;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
  lr?: boolean;
  colorA?: string;
  colorB?: string;
}

export default class CustomCard<T = { [key: string]: any }> extends React.Component<CardProps> {
  public render() {
    const { children, className, lr, hoverEffect, style, ...rest } = this.props;
    return (
      <Card
        className={`${className ?? ''}${hoverEffect ? ' card-hover' : ''}${lr ? ' lr' : ''}`}
        style={style}
        {...rest}
      >
        {children}
      </Card>
    );
  }
}
