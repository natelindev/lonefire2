/** @jsx jsx */
import Card from '../components/Card';
import { css, jsx } from '@emotion/core';

export default { title: 'HoverIntent' };

export const hoverIntent = () => (
  <Card
    className="border-draw-within mx-auto my-2"
    width="20rem"
    css={css`
      font-family: 'Economica', sans-serif;
    `}
    href="/"
    onMouseOver={(element: HTMLElement) => {
      element.classList.remove('active');
      element.classList.add('temp');
    }}
    onMouseOut={(element: HTMLElement) => {
      element.classList.add('active');
      element.classList.remove('temp');
    }}
  >
    <h1 className="mx-auto my-4">Nathaniel's Dev Area</h1>
  </Card>
);
