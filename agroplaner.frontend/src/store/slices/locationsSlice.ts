import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { locationService, type Location, type CreateLocationRequest, type UpdateLocationRequest } from '../../services/locationService';

interface LocationsState {
  locations: Location[];
  currentLocation: Location | null;
  loading: boolean;
  error: string | null;
}

const initialState: LocationsState = {
  locations: [],
  currentLocation: null,
  loading: false,
  error: null
};

// Async thunks
export const fetchLocations = createAsyncThunk<Location[], void, { rejectValue: string }>(
  'locations/fetchAll',
  async (_, { rejectWithValue }) => {
    try {
      return await locationService.getLocations();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch locations');
    }
  }
);

export const fetchLocation = createAsyncThunk<Location, number, { rejectValue: string }>(
  'locations/fetchOne',
  async (id, { rejectWithValue }) => {
    try {
      return await locationService.getLocation(id);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch location');
    }
  }
);

export const createLocation = createAsyncThunk<Location, CreateLocationRequest, { rejectValue: string }>(
  'locations/create',
  async (locationData, { rejectWithValue }) => {
    try {
      return await locationService.createLocation(locationData);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create location');
    }
  }
);

export const updateLocation = createAsyncThunk<
  Location,
  { id: number; data: UpdateLocationRequest },
  { rejectValue: string }
>(
  'locations/update',
  async ({ id, data }, { rejectWithValue }) => {
    try {
      return await locationService.updateLocation(id, data);
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update location');
    }
  }
);

export const deleteLocation = createAsyncThunk<number, number, { rejectValue: string }>(
  'locations/delete',
  async (id, { rejectWithValue }) => {
    try {
      await locationService.deleteLocation(id);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to delete location');
    }
  }
);

// Slice
const locationsSlice = createSlice({
  name: 'locations',
  initialState,
  reducers: {
    clearLocationError(state) {
      state.error = null;
    },
    clearCurrentLocation(state) {
      state.currentLocation = null;
    }
  },
  extraReducers: (builder) => {
    builder
      // Fetch all locations
      .addCase(fetchLocations.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchLocations.fulfilled, (state, action) => {
        state.loading = false;
        state.locations = action.payload;
      })
      .addCase(fetchLocations.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch locations';
      })
      
      // Fetch single location
      .addCase(fetchLocation.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchLocation.fulfilled, (state, action) => {
        state.loading = false;
        state.currentLocation = action.payload;
      })
      .addCase(fetchLocation.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to fetch location';
      })
      
      // Create location
      .addCase(createLocation.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createLocation.fulfilled, (state, action) => {
        state.loading = false;
        state.locations.push(action.payload);
        state.currentLocation = action.payload;
      })
      .addCase(createLocation.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to create location';
      })
      
      // Update location
      .addCase(updateLocation.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateLocation.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.locations.findIndex(loc => loc.locationId === action.payload.locationId);
        if (index !== -1) {
          state.locations[index] = action.payload;
        }
        state.currentLocation = action.payload;
      })
      .addCase(updateLocation.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to update location';
      })
      
      // Delete location
      .addCase(deleteLocation.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteLocation.fulfilled, (state, action) => {
        state.loading = false;
        state.locations = state.locations.filter(loc => loc.locationId !== action.payload);
        if (state.currentLocation?.locationId === action.payload) {
          state.currentLocation = null;
        }
      })
      .addCase(deleteLocation.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || 'Failed to delete location';
      });
  }
});

export const { clearLocationError, clearCurrentLocation } = locationsSlice.actions;
export default locationsSlice.reducer;
