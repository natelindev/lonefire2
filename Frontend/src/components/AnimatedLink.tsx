import * as React from 'react';
import './AnimatedLink.scoped.scss';

interface IAnimatedLinkProps {
  [key: string]: any;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
}

const AnimatedLink: React.FunctionComponent<IAnimatedLinkProps> = props => {
  const { className, children, ...rest } = props;
  return (
    <a className={`${className ?? ''} animated-link`} {...rest}>
      {children}
    </a>
  );
};

export default AnimatedLink;
