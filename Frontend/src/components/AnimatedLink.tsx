import * as React from 'react';
import './AnimatedLink.scoped.scss';

interface AnimatedLinkProps {
  [key: string]: any;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
}

const AnmiatedLink: React.SFC<AnimatedLinkProps> = (props: AnimatedLinkProps) => {
  const { className, children, ...rest } = props;
  return (
    <a className={`${className ?? ''} animated-link`} {...rest}>
      {children}
    </a>
  );
};

export default AnmiatedLink;
