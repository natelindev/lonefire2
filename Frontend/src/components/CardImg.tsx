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
  left?: boolean;
  right?: boolean;
  src?: string;
  width?: string;
  height?: string;
  alt?: string;
}

export default (props: CardImgProps)=> {
  const { children, className, left, right, ...rest } = props;
  return (
    <React.Fragment>
      <CardImg
        className={`${className ?? ''} ${left ? ' card-img-left' : ''} ${
          right ? ' card-img-right' : ''
          }`}
        {...rest}
      />
      {children ? <CardImgOverlay>{children}</CardImgOverlay> : null}
    </React.Fragment>
  );
}
