import * as React from 'react';
import { Card } from 'reactstrap';

export interface CardProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
  colorA?: string;
  colorB?: string;
}

export default class CustomCard<T = { [key: string]: any }> extends React.Component<CardProps> {
  public render() {
    const { children, className, style, ...rest } = this.props;
    return (
      <Card className={className} style={style} {...rest}>
        {children}
      </Card>
    );
  }
}
