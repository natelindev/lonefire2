import { Action, Reducer } from 'redux';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface UserState {
  loggedIn: boolean;
  userName: string;
  role: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

export interface LoginAction {
  type: 'LOGIN';
}
export interface LogoutAction {
  type: 'LOGOUT';
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type KnownAction = LoginAction | LogoutAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
  login: () => ({ type: 'LOGIN' } as LoginAction),
  logout: () => ({ type: 'LOGOUT' } as LogoutAction),
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<UserState> = (
  state: UserState | undefined,
  incomingAction: Action
): UserState => {
  if (state === undefined) {
    return {
      loggedIn: false,
      userName: '',
      role: 'nobody',
    };
  }

  const action = incomingAction as KnownAction;
  switch (action.type) {
    case 'LOGIN':
      return {
        loggedIn: true,
        userName: 'getUserName', // TODO: Get user name here
        role: 'role', // TODO:Get user role here
      };
    case 'LOGOUT':
      return {
        loggedIn: false,
        userName: '',
        role: 'nobody',
      };
    default:
      return state;
  }
};
