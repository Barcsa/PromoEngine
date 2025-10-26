import { SubmissionRequest, SubmissionResponse } from "../types/submission";

const API_BASE_URL = "http://localhost:5299/api";

export async function submitPromoCode(
    data: SubmissionRequest
): Promise<SubmissionResponse> {
    const response = await fetch(`${API_BASE_URL}/submission`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        return await response.json();
    }

    return await response.json();
}
