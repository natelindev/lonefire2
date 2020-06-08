import * as React from 'react';
import { CardImg as BSCardImg, CardImgOverlay } from 'reactstrap';
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

const CardImg: React.SFC<CardImgProps> = (props: CardImgProps) => {
  const { children, className, left, right, ...rest } = props;
  return (
    <>
      <BSCardImg
        className={`${className ?? ''} ${left ? ' card-img-left' : ''} ${
          right ? ' card-img-right' : ''
        }`}
        {...rest}
      />
      {children ? <CardImgOverlay>{children}</CardImgOverlay> : null}
    </>
  );
};

export default CardImg;
