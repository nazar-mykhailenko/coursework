import api from './api';

export interface Crop {
  id: number;
  name: string;
  plantId: number;
  locationId: number;
  expectedYield: number;
  cumulativeGDDToday: number;
  fieldArea: number;
  // Extended properties for detailed view
  plantingDate?: string;
  plant?: {
    plantId: number;
    name: string;
    description?: string;
  };
  location?: {
    locationId: number;
    name: string;
    latitude: number;
    longitude: number;
  };
}

export interface CropsResult {
  items: Crop[];
  totalCount: number;
}

export interface CreateCropDto {
  name: string;
  plantId: number;
  locationId: number;
  expectedYield: number;
  fieldArea: number;
}

export interface UpdateCropDto {
  id: number;
  name: string;
  expectedYield: number;
  fieldArea: number;
}

// Full prediction data interface
export interface FullPredictionResponse {
  irrigationAmount: number;
  nextIrrigationDate?: string;
  nitrogenFertilizerAmount: number;
  phosphorusFertilizerAmount: number;
  potassiumFertilizerAmount: number;
  nextFertilizationDate?: string;
  predictedHarvestDate?: string;
}

export const cropService = {
  async getCrops(pageNumber?: number, pageSize?: number): Promise<CropsResult> {
    let url = '/crops';
    if (pageNumber !== undefined && pageSize !== undefined) {
      url += `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    }
    const response = await api.get<CropsResult>(url);
    return response.data;
  },

  async getCrop(id: number): Promise<Crop> {
    const response = await api.get<Crop>(`/crops/${id}`);
    return response.data;
  },

  async createCrop(data: CreateCropDto): Promise<Crop> {
    const response = await api.post<Crop>('/crops', data);
    return response.data;
  },

  async updateCrop(id: number, data: UpdateCropDto): Promise<Crop> {
    const response = await api.put<Crop>(`/crops/${id}`, data);
    return response.data;
  },

  async deleteCrop(id: number): Promise<void> {
    await api.delete(`/crops/${id}`);
  },

  // Get full prediction for a crop
  async getFullPrediction(cropId: number): Promise<FullPredictionResponse> {
    const response = await api.get<FullPredictionResponse>(`/predictions/full/${cropId}`);
    return response.data;
  }
};
