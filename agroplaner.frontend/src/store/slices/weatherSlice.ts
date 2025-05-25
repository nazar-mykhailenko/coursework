import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { weatherService } from '../../services/weatherService';

interface WeatherState {
  loading: boolean;
  error: string | null;
  lastUpdateMessage: string | null;
}

const initialState: WeatherState = {
  loading: false,
  error: null,
  lastUpdateMessage: null
};

// Async thunks
export const updateAllLocationsWeather = createAsyncThunk<
  { message: string },
  void,
  { rejectValue: string }
>(
  'weather/updateAll',
  async (_, { rejectWithValue }) => {
    try {
      return await weatherService.updateAllLocationsWeather();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update weather data');
    }
  }
);

// Slice
const weatherSlice = createSlice({
  name: 'weather',
  initialState,
  reducers: {
    clearWeatherError(state) {
      state.error = null;
    },
    clearUpdateMessage(state) {
      state.lastUpdateMessage = null;
    }
  },
  extraReducers: (builder) => {
    builder
      // Update all locations weather
      .addCase(updateAllLocationsWeather.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateAllLocationsWeather.fulfilled, (state, action) => {
        state.loading = false;
        state.lastUpdateMessage = action.payload.message;
      })
      .addCase(updateAllLocationsWeather.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to update weather data';
      });
  }
});

export const { clearWeatherError, clearUpdateMessage } = weatherSlice.actions;
export default weatherSlice.reducer;
