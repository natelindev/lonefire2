import * as React from 'react';
import { Button } from 'reactstrap';

export interface GradientButtonProps extends React.Props<any> {
  children?: React.ReactNode;
  className?: string;
  style?: any;
  colorA: string;
  colorB: string;
}

const isBright = (hexColor: string) => {
  const [r, g, b] = [0, 2, 4].map(p => parseInt(hexColor.substr(p, 2), 16));
  return (r * 299 + g * 587 + b * 114) / 1000 >= 128;
};

export default class GradientButton extends React.Component<GradientButtonProps> {
  public render() {
    const { colorA, colorB, children, className, style } = this.props;
    return (
      <Button
        color="primary"
        className={className}
        style={{
          border: 'none',
          background: `linear-gradient(45deg, ${colorA} 0%, ${colorB} 90%)`,
          color: isBright(colorA) && isBright(colorB) ? '#000000' : '#ffffff',
          ...style
        }}
      >
        {children}
      </Button>
    );
  }
}
