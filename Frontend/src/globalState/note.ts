import { atom } from 'recoil';

export const notesState = atom({
  key: 'notes',
  default: [],
});
