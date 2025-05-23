import axios from 'axios';

const API_URL = 'http://localhost:5105/api'; // adjust as needed for your backend URL

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
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await axios.get<Plant[]>(`${API_URL}/plants`, {
      headers: { Authorization: `Bearer ${token}` }
    });
    return response.data;
  },

  async predictSeedingDate(data: SeedingPredictionRequest): Promise<SeedingPredictionResponse> {
    const token = localStorage.getItem('token');
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await axios.post<SeedingPredictionResponse>(
      `${API_URL}/predictions/seeding`,
      data,
      {
        headers: { Authorization: `Bearer ${token}` }
      }
    );
    return response.data;
  }
};
