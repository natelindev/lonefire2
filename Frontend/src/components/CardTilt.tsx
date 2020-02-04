import React, { useRef, useState } from 'react';
import { Canvas, useFrame } from 'react-three-fiber';
import { DoubleSide } from 'three';

export default function Box(props: any) {
  // This reference will give us direct access to the mesh
  const mesh: any = useRef();
  // Set up state for the hovered and active state
  const [hovered, setHover] = useState(false);
  const mouseTolerance = 0.2;

  let centerX = window.innerWidth * 0.5;
  let centerY = window.innerHeight * 0.5;
  let clientX = centerX;
  let clientY = centerY;

  // Rotate mesh every frame, this is outside of React without overhead
  useFrame(() => {
    document.onmousemove = e => {
      clientX = e.clientX;
      clientY = e.clientY;
    };
    mesh.current.rotation.y = ((clientX - centerX) / centerX) * mouseTolerance;
    mesh.current.rotation.x = ((clientY - centerY) / centerY) * mouseTolerance;
  });

  return (
    <mesh
      {...props}
      ref={mesh}
      onPointerOver={e => {
        setHover(true);
      }}
      onPointerOut={e => setHover(false)}
    >
      <planeBufferGeometry attach="geometry" args={[20, 20, 32]} />
      <meshBasicMaterial attach="material" color={0xffff00} side={DoubleSide} />
    </mesh>
  );
}
