
const API_BASE_URL = process.env.QUOTIFYAI_API_URL;

export class ApiService {
    private static async fetchApi<T>(
        endpoint: string,
        options: RequestInit = {}
    ): Promise<ApiResponse<T>> {
        const defaultHeaders = {
            'Content-Type': 'application/json',
        };

        try {
            const response = await fetch(`${API_BASE_URL}${endpoint}`, {
                ...options,
                headers: {
                    ...defaultHeaders,
                    ...options.headers,
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            if (response.status === 204) {
                return { data: null as T };
            }

            const data = await response.json();
            return { data };
        } catch (error) {
            console.error('API request failed:', error);
            return {
                data: null as T,
                error: error instanceof Error ? error.message : 'Unknown error occurred',
            };
        }
    }

    static async get<T>(endpoint: string): Promise<ApiResponse<T>> {
        return this.fetchApi<T>(endpoint, { method: 'GET' });
    }

    static async post<T>(endpoint: string, body: any): Promise<ApiResponse<T>> {
        return this.fetchApi<T>(endpoint, {
            method: 'POST',
            body: JSON.stringify(body),
        });
    }

    static async put<T>(endpoint: string, body: any): Promise<ApiResponse<T>> {
        return this.fetchApi<T>(endpoint, {
            method: 'PUT',
            body: JSON.stringify(body),
        });
    }

    static async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
        return this.fetchApi<T>(endpoint, { method: 'DELETE' });
    }
}