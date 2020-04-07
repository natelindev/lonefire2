import * as React from 'react';
import './AnimatedLink.scoped.scss';

interface IAnimatedLinkProps {
  [key: string]: any;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
}

export default (props: IAnimatedLinkProps) => {
  const { className, children, ...rest } = props;
  return (
    <a className={`${className ?? ''} animated-link`} {...rest}>
      {children}
    </a>
  );
};

