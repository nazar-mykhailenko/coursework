import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { 
  predictionService, 
  type Plant, 
  type SeedingPredictionRequest, 
  type SeedingPredictionResponse 
} from '../../services/predictionService';

interface PredictionState {
  plants: Plant[];
  predictionResult: SeedingPredictionResponse | null;
  loading: boolean;
  plantsLoading: boolean;
  error: string | null;
}

const initialState: PredictionState = {
  plants: [],
  predictionResult: null,
  loading: false,
  plantsLoading: false,
  error: null
};

// Async thunks
export const fetchPlants = createAsyncThunk<Plant[], void, { rejectValue: string }>(
  'predictions/fetchPlants',
  async (_, { rejectWithValue }) => {
    try {
      return await predictionService.getAllPlants();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch plants');
    }
  }
);

export const predictSeedingDate = createAsyncThunk<
  SeedingPredictionResponse, 
  SeedingPredictionRequest, 
  { rejectValue: string }
>(
  'predictions/seedingDate',
  async (requestData, { rejectWithValue }) => {
    try {
      return await predictionService.predictSeedingDate(requestData);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to get seeding prediction');
    }
  }
);

const predictionsSlice = createSlice({
  name: 'predictions',
  initialState,
  reducers: {
    clearPredictionResult: (state) => {
      state.predictionResult = null;
    },
    clearError: (state) => {
      state.error = null;
    }
  },
  extraReducers: (builder) => {
    builder
      // Fetch plants
      .addCase(fetchPlants.pending, (state) => {
        state.plantsLoading = true;
        state.error = null;
      })
      .addCase(fetchPlants.fulfilled, (state, action: PayloadAction<Plant[]>) => {
        state.plantsLoading = false;
        state.plants = action.payload;
      })
      .addCase(fetchPlants.rejected, (state, action) => {
        state.plantsLoading = false;
        state.error = action.payload || 'Failed to fetch plants';
      })
      
      // Predict seeding date
      .addCase(predictSeedingDate.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.predictionResult = null;
      })
      .addCase(predictSeedingDate.fulfilled, (state, action: PayloadAction<SeedingPredictionResponse>) => {
        state.loading = false;
        state.predictionResult = action.payload;
      })
      .addCase(predictSeedingDate.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to get prediction';
      });
  }
});

export const { clearPredictionResult, clearError } = predictionsSlice.actions;
export default predictionsSlice.reducer;
