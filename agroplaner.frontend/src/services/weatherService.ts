import api from './api';

export const weatherService = {
  async updateAllLocationsWeather(): Promise<{ message: string }> {
    const response = await api.post<{ message: string }>('/weather/update-all');
    return response.data;
  }
};
