import React from 'react';

import { css } from '@emotion/core';

import Card from '../components/Card';

const Home = (): React.ReactElement => (
  <div>
    <Card
      className="border-draw-within mx-auto my-2"
      width="20rem"
      css={css`
        font-family: 'Economica', sans-serif;
        backdrop-filter: saturate(180%) blur(5px);
      `}
      href="/"
    >
      <h1 className="mx-auto my-4">Nathaniel&#39;s Dev Area</h1>
    </Card>
  </div>
);

export default Home;
