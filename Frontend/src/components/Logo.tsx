import * as React from 'react';

type LogoProps = {
  size: string;
  height?: string;
  width?: string;
};
export default class Logo extends React.PureComponent<LogoProps> {
  static defaultProps = {
    size: '2rem'
  };
  public render() {
    const { size, height, width } = this.props;
    return (
      <img
        className="logo"
        style={{ maxHeight: size || height, maxWidth: size || width }}
        src="logo.svg"
        alt="logo"
      ></img>
    );
  }
}
