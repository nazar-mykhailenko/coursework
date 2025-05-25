import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { 
  cropService, 
  type Crop, 
  type CropsResult, 
  type CreateCropDto, 
  type UpdateCropDto 
} from '../../services/cropService';

interface CropsState {
  crops: Crop[];
  currentCrop: Crop | null;
  totalCount: number;
  loading: boolean;
  error: string | null;
}

const initialState: CropsState = {
  crops: [],
  currentCrop: null,
  totalCount: 0,
  loading: false,
  error: null
};

// Async thunks
export const fetchCrops = createAsyncThunk<
  CropsResult, 
  { pageNumber?: number, pageSize?: number } | void, 
  { rejectValue: string }
>(
  'crops/fetchAll',
  async (params, { rejectWithValue }) => {
    try {
      const pageNumber = params?.pageNumber;
      const pageSize = params?.pageSize;
      return await cropService.getCrops(pageNumber, pageSize);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch crops');
    }
  }
);

export const fetchCrop = createAsyncThunk<Crop, number, { rejectValue: string }>(
  'crops/fetchOne',
  async (id, { rejectWithValue }) => {
    try {
      return await cropService.getCrop(id);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch crop');
    }
  }
);

export const createCrop = createAsyncThunk<Crop, CreateCropDto, { rejectValue: string }>(
  'crops/create',
  async (cropData, { rejectWithValue }) => {
    try {
      return await cropService.createCrop(cropData);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create crop');
    }
  }
);

export const updateCrop = createAsyncThunk<
  Crop,
  { id: number; data: UpdateCropDto },
  { rejectValue: string }
>(
  'crops/update',
  async ({ id, data }, { rejectWithValue }) => {
    try {
      return await cropService.updateCrop(id, data);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update crop');
    }
  }
);

export const deleteCrop = createAsyncThunk<number, number, { rejectValue: string }>(
  'crops/delete',
  async (id, { rejectWithValue }) => {
    try {
      await cropService.deleteCrop(id);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to delete crop');
    }
  }
);

// Slice
const cropsSlice = createSlice({
  name: 'crops',
  initialState,
  reducers: {
    clearCropError(state) {
      state.error = null;
    },
    clearCurrentCrop(state) {
      state.currentCrop = null;
    }
  },
  extraReducers: (builder) => {
    builder
      // Fetch all crops
      .addCase(fetchCrops.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCrops.fulfilled, (state, action) => {
        state.loading = false;
        state.crops = action.payload.items;
        state.totalCount = action.payload.totalCount;
      })
      .addCase(fetchCrops.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch crops';
      })
      
      // Fetch single crop
      .addCase(fetchCrop.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCrop.fulfilled, (state, action) => {
        state.loading = false;
        state.currentCrop = action.payload;
      })
      .addCase(fetchCrop.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch crop';
      })
      
      // Create crop
      .addCase(createCrop.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createCrop.fulfilled, (state, action) => {
        state.loading = false;
        state.crops.push(action.payload);
        state.totalCount += 1;
        state.currentCrop = action.payload;
      })
      .addCase(createCrop.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to create crop';
      })
      
      // Update crop
      .addCase(updateCrop.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateCrop.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.crops.findIndex(crop => crop.id === action.payload.id);
        if (index !== -1) {
          state.crops[index] = action.payload;
        }
        state.currentCrop = action.payload;
      })
      .addCase(updateCrop.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to update crop';
      })
      
      // Delete crop
      .addCase(deleteCrop.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteCrop.fulfilled, (state, action) => {
        state.loading = false;
        state.crops = state.crops.filter(crop => crop.id !== action.payload);
        state.totalCount -= 1;
        if (state.currentCrop?.id === action.payload) {
          state.currentCrop = null;
        }
      })
      .addCase(deleteCrop.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to delete crop';
      });
  }
});

export const { clearCropError, clearCurrentCrop } = cropsSlice.actions;
export default cropsSlice.reducer;
