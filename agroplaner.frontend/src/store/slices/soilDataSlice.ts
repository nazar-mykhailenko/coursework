import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { 
  soilDataService, 
  type SoilData, 
  type IrrigationEventDto, 
  type FertilizationEventDto
} from '../../services/soilDataService';

interface SoilDataState {
  currentSoilData: SoilData | null;
  loading: boolean;
  error: string | null;
}

const initialState: SoilDataState = {
  currentSoilData: null,
  loading: false,
  error: null
};

// Async thunks
export const fetchSoilDataByCrop = createAsyncThunk<SoilData, number, { rejectValue: string }>(
  'soilData/fetchByCrop',
  async (cropId, { rejectWithValue }) => {
    try {
      return await soilDataService.getSoilDataByCrop(cropId);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch soil data');
    }
  }
);

export const updateSoilData = createAsyncThunk<SoilData, SoilData, { rejectValue: string }>(
  'soilData/update',
  async (data, { rejectWithValue }) => {
    try {
      return await soilDataService.updateSoilData(data);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update soil data');
    }
  }
);

export const applyIrrigation = createAsyncThunk<
  SoilData,
  { cropId: number; data: IrrigationEventDto },
  { rejectValue: string }
>(
  'soilData/applyIrrigation',
  async ({ cropId, data }, { rejectWithValue }) => {
    try {
      return await soilDataService.applyIrrigation(cropId, data);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to apply irrigation');
    }
  }
);

export const applyFertilization = createAsyncThunk<
  SoilData,
  { cropId: number; data: FertilizationEventDto },
  { rejectValue: string }
>(
  'soilData/applyFertilization',
  async ({ cropId, data }, { rejectWithValue }) => {
    try {
      return await soilDataService.applyFertilization(cropId, data);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to apply fertilization');
    }
  }
);

// Slice
const soilDataSlice = createSlice({
  name: 'soilData',
  initialState,
  reducers: {
    clearSoilDataError(state) {
      state.error = null;
    },
    clearCurrentSoilData(state) {
      state.currentSoilData = null;
    }
  },
  extraReducers: (builder) => {
    builder
      // Fetch soil data by crop
      .addCase(fetchSoilDataByCrop.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchSoilDataByCrop.fulfilled, (state, action) => {
        state.loading = false;
        state.currentSoilData = action.payload;
      })
      .addCase(fetchSoilDataByCrop.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch soil data';
      })
      
      // Update soil data
      .addCase(updateSoilData.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateSoilData.fulfilled, (state, action) => {
        state.loading = false;
        state.currentSoilData = action.payload;
      })
      .addCase(updateSoilData.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to update soil data';
      })
      
      // Apply irrigation
      .addCase(applyIrrigation.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(applyIrrigation.fulfilled, (state, action) => {
        state.loading = false;
        state.currentSoilData = action.payload;
      })
      .addCase(applyIrrigation.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to apply irrigation';
      })
      
      // Apply fertilization
      .addCase(applyFertilization.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(applyFertilization.fulfilled, (state, action) => {
        state.loading = false;
        state.currentSoilData = action.payload;
      })
      .addCase(applyFertilization.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to apply fertilization';
      });
  }
});

export const { clearSoilDataError, clearCurrentSoilData } = soilDataSlice.actions;
export default soilDataSlice.reducer;
