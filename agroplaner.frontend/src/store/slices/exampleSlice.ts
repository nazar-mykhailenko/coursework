import { createSlice, type PayloadAction } from '@reduxjs/toolkit';

// Define a type for the slice state
interface ExampleState {
  value: number;
  status: 'idle' | 'loading' | 'failed';
}

// Define the initial state using that type
const initialState: ExampleState = {
  value: 0,
  status: 'idle',
};

export const exampleSlice = createSlice({
  name: 'example',
  initialState,
  reducers: {
    // Define reducers here
    increment: (state) => {
      state.value += 1;
    },
    decrement: (state) => {
      state.value -= 1;
    },
    // Use the PayloadAction type to declare the contents of `action.payload`
    incrementByAmount: (state, action: PayloadAction<number>) => {
      state.value += action.payload;
    },
  },
});

// Export actions
export const { increment, decrement, incrementByAmount } = exampleSlice.actions;

// Export reducer
export default exampleSlice.reducer;
