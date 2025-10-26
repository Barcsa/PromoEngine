import React, { useState } from "react";
import FormInput from "../components/FormInput";
import Checkbox from "../components/Checkbox";
import Modal from "../components/Modal";
import { submitPromoCode } from "../api/submissionApi";
import { SubmissionRequest } from "../types/submission";

const SubmissionForm: React.FC = () => {
    const [formData, setFormData] = useState<SubmissionRequest>({
        firstName: "",
        lastName: "",
        email: "",
        promoCode: "",
        acceptedPrivacyPolicy: false,
        acceptedGameRules: false,
    });

    const [loading, setLoading] = useState(false);
    const [modalMessage, setModalMessage] = useState("");
    const [isModalOpen, setIsModalOpen] = useState(false);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        const { name, value, type, checked } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: type === "checkbox" ? checked : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!formData.firstName || !formData.lastName || !formData.email || !formData.promoCode) {
            setModalMessage("Kérlek, töltsd ki az összes kötelező mezőt!");
            setIsModalOpen(true);
            return;
        }

        if (!/\S+@\S+\.\S+/.test(formData.email)) {
            setModalMessage("Az e-mail cím formátuma érvénytelen.");
            setIsModalOpen(true);
            return;
        }

        if (!formData.acceptedGameRules || !formData.acceptedPrivacyPolicy) {
            setModalMessage("A beküldéshez el kell fogadnod a szabályzatokat!");
            setIsModalOpen(true);
            return;
        }

        setLoading(true);
        try {
            const result = await submitPromoCode(formData);
            setModalMessage(result.message);
        } catch {
            setModalMessage("Hiba történt a beküldés során. Próbáld újra!");
        } finally {
            setLoading(false);
            setIsModalOpen(true);
        }

        console.log("request:", formData);

    };

    return (
        <div className="min-h-screen bg-[#1b2a3a] flex items-center justify-center px-4">
            <form
                onSubmit={handleSubmit}
                noValidate
                className="bg-[#22384d] p-8 rounded-xl shadow-2xl w-full max-w-2xl text-white"
            >
                <h1 className="text-3xl font-bold mb-2 text-center">KÓDFELTÖLTÉS</h1>
                <p className="text-center text-gray-300 mb-8">
                    Add meg az adataidat, és töltsd fel a csomagoláson található kódot!
                </p>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormInput
                        label="Vezetéknév*"
                        name="lastName"
                        value={formData.lastName}
                        onChange={handleChange}
                    />
                    <FormInput
                        label="Keresztnév*"
                        name="firstName"
                        value={formData.firstName}
                        onChange={handleChange}
                    />
                    <FormInput
                        label="Email cím*"
                        name="email"
                        type="email"
                        value={formData.email}
                        onChange={handleChange}
                    />
                    <FormInput
                        label="Promóciós kód*"
                        name="promoCode"
                        value={formData.promoCode}
                        onChange={handleChange}
                    />
                </div>

                <div className="mt-6 space-y-3">
                    <Checkbox
                        label={
                            <>
                                Elfogadom a{" "}
                                <a href="#" className="underline hover:text-indigo-400">
                                    játékszabályzatot
                                </a>
                            </>
                        }
                        name="acceptedGameRules"
                        checked={formData.acceptedGameRules}
                        onChange={handleChange}
                    />
                    <Checkbox
                        label={
                            <>
                                Elfogadom az{" "}
                                <a href="#" className="underline hover:text-indigo-400">
                                    adatvédelmi szabályzatot
                                </a>
                            </>
                        }
                        name="acceptedPrivacyPolicy"
                        checked={formData.acceptedPrivacyPolicy}
                        onChange={handleChange}
                    />
                </div>

                <p className="text-sm text-gray-400 mt-4">
                    Több kód feltöltésével növelheted nyerési esélyeidet.
                </p>

                <button
                    type="submit"
                    disabled={loading}
                    className={`w-full mt-6 py-3 rounded-md font-semibold transition ${loading
                        ? "bg-gray-500 cursor-not-allowed"
                        : "bg-indigo-500 hover:bg-indigo-600"
                        }`}
                >
                    {loading ? "Feltöltés..." : "Feltöltöm a kódot"}
                </button>
            </form>

            <Modal
                isOpen={isModalOpen}
                message={modalMessage}
                onClose={() => setIsModalOpen(false)}
            />
        </div>
    );
};

export default SubmissionForm;
