import * as React from 'react';

export interface LogoProps {
  size?: string;
  height?: string;
  width?: string;
}

export default (props: LogoProps) => (
  <img
    className="logo"
    style={{
      maxHeight: props.size || props.height || '2rem',
      maxWidth: props.size || props.width || '2rem',
      margin: '0.3rem'
    }}
    src="logo.svg"
    alt="logo"
  ></img>
);
