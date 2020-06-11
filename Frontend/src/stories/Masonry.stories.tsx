import React from 'react';
import { css } from '@emotion/core';
import Masonry from '../components/Masonry';
import { rng, RngOption } from '../util/randomGenerator';

export default { title: 'Masonry' };

export const basic: React.SFC = () => (
  <Masonry
    packed="packed"
    sizes={[
      { columns: 1, gutter: 20 },
      { mq: '768px', columns: 2, gutter: 20 },
      { mq: '1024px', columns: 3, gutter: 20 },
    ]}
    position
  >
    {rng(RngOption.array, 400, 200, 20).map((height, index) => (
      <div
        key={height}
        css={css`
          width: 200px;
          height: ${height}px;
          border: 1px solid black;
          padding: 4px 4px;
        `}
      >
        Element {index + 1}
      </div>
    ))}
  </Masonry>
);
