import api from './api';

// Updating Plant interface to align with the one in predictionService.ts 
// while preserving backend properties
export interface Plant {
  plantId: number;
  name: string;
  // Properties from predictionService
  growthDuration?: number;
  nitrogenNeed?: number;
  phosphorusNeed?: number;
  potassiumNeed?: number;
  optimalSoilMoisture?: number;
  optimalSoilTemperature?: number;
  description?: string;
  
  // Properties from backend
  minSoilTempForSeeding: number;
  baseTempForGDD: number;
  maturityGDD: number;
  rootDepth: number;
  allowableDepletionFraction: number;
  nitrogenContent: number;
  phosphorusContent: number;
  potassiumContent: number;
}

export const plantService = {
  async getPlants(): Promise<Plant[]> {
    const response = await api.get<Plant[]>('/plants');
    return response.data;
  },

  async getPlant(id: number): Promise<Plant> {
    const response = await api.get<Plant>(`/plants/${id}`);
    return response.data;
  }
};
