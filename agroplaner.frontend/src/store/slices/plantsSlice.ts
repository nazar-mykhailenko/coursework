import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { plantService, type Plant } from '../../services/plantService';

interface PlantsState {
  plants: Plant[];
  currentPlant: Plant | null;
  loading: boolean;
  error: string | null;
}

const initialState: PlantsState = {
  plants: [],
  currentPlant: null,
  loading: false,
  error: null
};

// Async thunks
export const fetchPlants = createAsyncThunk<Plant[], void, { rejectValue: string }>(
  'plants/fetchAll',
  async (_, { rejectWithValue }) => {
    try {
      return await plantService.getPlants();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch plants');
    }
  }
);

export const fetchPlant = createAsyncThunk<Plant, number, { rejectValue: string }>(
  'plants/fetchOne',
  async (id, { rejectWithValue }) => {
    try {
      return await plantService.getPlant(id);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch plant');
    }
  }
);

// Slice
const plantsSlice = createSlice({
  name: 'plants',
  initialState,
  reducers: {
    clearPlantError(state) {
      state.error = null;
    },
    clearCurrentPlant(state) {
      state.currentPlant = null;
    }
  },
  extraReducers: (builder) => {
    builder
      // Fetch all plants
      .addCase(fetchPlants.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPlants.fulfilled, (state, action) => {
        state.loading = false;
        state.plants = action.payload;
      })
      .addCase(fetchPlants.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch plants';
      })
      
      // Fetch single plant
      .addCase(fetchPlant.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPlant.fulfilled, (state, action) => {
        state.loading = false;
        state.currentPlant = action.payload;
      })
      .addCase(fetchPlant.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch plant';
      });
  }
});

export const { clearPlantError, clearCurrentPlant } = plantsSlice.actions;
export default plantsSlice.reducer;
