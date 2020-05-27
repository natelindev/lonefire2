import React from 'react';
import { connect } from 'react-redux';
import Card from '../components/Card';
import { css } from '@emotion/core';

const Home = () => (
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
      <h1 className="mx-auto my-4">Nathaniel's Dev Area</h1>
    </Card>
  </div>
);

export default connect()(Home);
