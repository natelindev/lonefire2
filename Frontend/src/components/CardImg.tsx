import * as React from 'react';
import { CardImg } from 'reactstrap';

export interface CardImgProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  className?: string;
  style?: React.CSSProperties;
  left?: boolean;
}

export default class CustomCardImg extends React.Component<CardImgProps> {
  public render() {
    const { className, left, ...rest } = this.props;
    return <CardImg className={`${className ?? ''} card-img-left`} {...rest}></CardImg>;
  }
}
