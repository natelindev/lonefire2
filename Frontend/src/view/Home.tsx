import React from 'react';

import { css } from '@emotion/core';

import Card from '../components/Card';
import Masonry from '../components/Masonry';
import { rng, RngOption } from '../util/randomGenerator';

const Home = (): React.ReactElement => (
  <div>
    <Card
      className="border-draw-within mx-auto my-3"
      width="20rem"
      css={css`
        font-family: 'Economica', sans-serif;
        backdrop-filter: saturate(180%) blur(5px);
      `}
      href="/"
    >
      <h1 className="mx-auto my-4">Nathaniel&#39;s Dev Area</h1>
    </Card>
    <div className="d-flex justify-content-center mt-5 mb-3">
      <Masonry
        packed="packed"
        sizes={[
          { columns: 1, gutter: 20 },
          { mq: '768px', columns: 2, gutter: 20 },
          { mq: '1024px', columns: 3, gutter: 20 },
        ]}
        position
        css={css`
          width: 100%;
          align-self: center;
          margin: 0 0;
        `}
      >
        {rng(RngOption.array, 400, 200, 20).map((height, index) => (
          <div
            key={height}
            css={css`
              width: 20rem;
              height: ${height}px;
              border: 1px solid black;
              padding: 4px 4px;
              margin: auto auto;
            `}
          >
            Article {index + 1}
          </div>
        ))}
      </Masonry>
    </div>
  </div>
);

export default Home;
