import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import { apiSlice } from '../services/apiSlice';
import exampleReducer from './slices/exampleSlice';

// Import additional reducers as needed
import authReducer from './slices/authSlice';
import locationsReducer from './slices/locationsSlice';
import predictionsReducer from './slices/predictionsSlice';
import cropsReducer from './slices/cropsSlice';
import plantsReducer from './slices/plantsSlice';
import soilDataReducer from './slices/soilDataSlice';

export const store = configureStore({  reducer: {
    // Core reducers
    example: exampleReducer,
    // API slice reducer
    [apiSlice.reducerPath]: apiSlice.reducer,
    // Add other reducers here as needed
    auth: authReducer,
    locations: locationsReducer,
    predictions: predictionsReducer,
    crops: cropsReducer,
    plants: plantsReducer,
    soilData: soilDataReducer,
  },
  middleware: (getDefaultMiddleware) => 
    getDefaultMiddleware({
      serializableCheck: false,
    }).concat(apiSlice.middleware),
});

// Optional, but required for refetchOnFocus/refetchOnReconnect behaviors
setupListeners(store.dispatch);

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
