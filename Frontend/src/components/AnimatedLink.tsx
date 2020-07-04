import './AnimatedLink.scoped.scss';

import * as React from 'react';

interface AnimatedLinkProps {
  [key: string]: any;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
}

const AnimatedLink: React.SFC<AnimatedLinkProps> = (props: AnimatedLinkProps) => {
  const { className, children, ...rest } = props;
  return (
    <a className={`${className ?? ''} animated-link`} {...rest}>
      {children}
    </a>
  );
};

export default AnimatedLink;
