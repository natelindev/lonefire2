import React, { useRef } from 'react';
import Bricks from 'bricks.js';

export default function Masonry() {
  const masonryContainter = useRef(null);

  return <div ref={masonryContainter}></div>;
}
