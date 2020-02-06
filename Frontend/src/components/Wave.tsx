/** @jsx jsx */
import * as React from 'react';
import './Wave.scoped.scss';
import { css, jsx } from '@emotion/core';

export interface WaveProps extends React.Props<any> {
  [key: string]: any;
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
  background?: string;
  color?: string;
}

const Wave = (props: WaveProps) => (
  <div
    className="wave-container"
    css={css`
      background: ${props.background ?? '#5cc6ee'};
      color: ${props.color ?? '#fff'};
    `}
  >
    {props.children}
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320">
      <path
        fill={props.color ?? '#fff'}
        fillOpacity="1"
        d="M0,32L48,32C96,32,192,32,288,64C384,96,480,160,576,154.7C672,149,768,75,864,64C960,53,1056,107,1152,117.3C1248,128,1344,96,1392,80L1440,64L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"
      ></path>
    </svg>
  </div>
);

export default Wave;
