import { atom } from 'recoil';

export const notificationsState = atom({
  key: 'notifications',
  default: [],
});
