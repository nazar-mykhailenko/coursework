import api from './api';

// SoilData interface matching backend DTO exactly
export interface SoilData {
  soilDataId: number;
  cropId: number;
  currentMoisture: number;
  fieldCapacity: number;
  temperature: number;
  availableNitrogen: number;
  availablePhosphorus: number;
  availablePotassium: number;
}

// DTOs matching backend exactly
export interface IrrigationEventDto {
  amount: number;
}

export interface FertilizationEventDto {
  nitrogenAmount: number;
  phosphorusAmount: number;
  potassiumAmount: number;
}

// Prediction responses for individual actions
export interface IrrigationPredictionResponse {
  amount: number;
  nextDate?: string;
}

export interface FertilizationPredictionResponse {
  nitrogenAmount: number;
  phosphorusAmount: number;
  potassiumAmount: number;
  nextDate?: string;
}

export const soilDataService = {
  async getSoilDataByCrop(cropId: number): Promise<SoilData> {
    const response = await api.get<SoilData>(`/soildata/crop/${cropId}`);
    return response.data;
  },

  // Backend only has PUT endpoint that creates if doesn't exist
  async updateSoilData(data: SoilData): Promise<SoilData> {
    const response = await api.put<SoilData>('/soildata', data);
    return response.data;
  },

  async applyIrrigation(cropId: number, data: IrrigationEventDto): Promise<SoilData> {
    const response = await api.post<SoilData>(`/soildata/crop/${cropId}/irrigation`, data);
    return response.data;
  },

  async applyFertilization(cropId: number, data: FertilizationEventDto): Promise<SoilData> {
    const response = await api.post<SoilData>(`/soildata/crop/${cropId}/fertilization`, data);
    return response.data;
  },

  // Get irrigation prediction for a crop
  async getIrrigationPrediction(cropId: number): Promise<IrrigationPredictionResponse> {
    const response = await api.get<IrrigationPredictionResponse>(`/predictions/irrigation/${cropId}`);
    return response.data;
  },

  // Get fertilization prediction for a crop
  async getFertilizationPrediction(cropId: number): Promise<FertilizationPredictionResponse> {
    const response = await api.get<FertilizationPredictionResponse>(`/predictions/fertilization/${cropId}`);
    return response.data;
  }
};
