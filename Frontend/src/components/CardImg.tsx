import * as React from 'react';
import { CardImg, CardImgOverlay } from 'reactstrap';
import './CardImg.scoped.scss';

export interface CardImgProps extends React.HTMLAttributes<HTMLElement> {
  [key: string]: any;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
  top?: boolean;
  bottom?: boolean;
  src?: string;
  width?: string;
  height?: string;
  alt?: string;
}

export default class CustomCardImg extends React.Component<CardImgProps> {
  public render() {
    const { className, children, left, right, ...rest } = this.props;
    return (
      <React.Fragment>
        <CardImg className={className} {...rest} />
        {children ? <CardImgOverlay>{children}</CardImgOverlay> : null}
      </React.Fragment>
    );
  }
}
