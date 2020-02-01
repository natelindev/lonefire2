/** @jsx jsx */
import * as React from 'react';
import { Button } from 'reactstrap';
import { css, jsx } from '@emotion/core';
import './GradientButton.scoped.scss';

export interface GradientButtonProps extends React.Props<any> {
  children?: React.ReactNode;
  className?: string;
  style?: React.CSSProperties;
  colorA: string;
  colorB: string;
}

const isBright = (hexColor: string) => {
  const [r, g, b] = [0, 2, 4].map(p => parseInt(hexColor.substr(p, 2), 16));
  return (r * 299 + g * 587 + b * 114) / 1000 >= 128;
};

export default class GradientButton extends React.Component<GradientButtonProps> {
  public render() {
    const { colorA, colorB, children, className, ...rest } = this.props;
    return (
      <Button
        color="primary"
        className={`${className ?? ''} gradient-button`}
        css={css`
          background-image: linear-gradient(45deg, ${colorA} 10%, ${colorB} 90%);
          color: ${isBright(colorA) && isBright(colorB) ? '#000000' : '#ffffff'};
          &::before {
            background-image: linear-gradient(45deg, ${colorA} 35%, ${colorB} 75%);
          }
        `}
        {...rest}
      >
        <div className="btn-content">{children}</div>
      </Button>
    );
  }
}
