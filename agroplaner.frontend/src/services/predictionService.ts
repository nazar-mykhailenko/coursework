import api from './api';

export interface Plant {
  plantId: number;
  name: string;
  growthDuration: number;
  nitrogenNeed: number;
  phosphorusNeed: number;
  potassiumNeed: number;
  optimalSoilMoisture: number;
  optimalSoilTemperature: number;
  description: string;
}

export interface SoilData {
  temperature: number;
  fieldCapacity: number;
  currentMoisture: number;
  availableNitrogen: number;
  availablePhosphorus: number;
  availablePotassium: number;
}

export interface SeedingPredictionRequest {
  plantId: number;
  latitude: number;
  longitude: number;
  expectedYield: number;
  fieldArea: number;
  soil: SoilData;
}

export interface SeedingPredictionResponse {
  recommendedSeedingDate: string;
}

export const predictionService = {
  async getAllPlants(): Promise<Plant[]> {
    const response = await api.get<Plant[]>('/plants');
    return response.data;
  },

  async predictSeedingDate(data: SeedingPredictionRequest): Promise<SeedingPredictionResponse> {
    const response = await api.post<SeedingPredictionResponse>('/predictions/seeding', data);
    return response.data;
  }
};
