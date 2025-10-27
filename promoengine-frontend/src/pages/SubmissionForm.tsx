import React, { useState } from "react";
import FormInput from "../components/FormInput";
import Checkbox from "../components/Checkbox";
import Modal from "../components/Modal";
import { submitPromoCode } from "../api/submissionApi";
import { SubmissionFormData } from "../types/submission";

const SubmissionForm: React.FC = () => {
    const [formData, setFormData] = useState<SubmissionFormData>({
        firstName: "",
        lastName: "",
        email: "",
        confirmEmail: "",
        phoneNumber: "",
        promoCode: "",
        acceptedPrivacyPolicy: false,
        acceptedGameRules: false,
    });

    const [errors, setErrors] = useState<Record<string, string>>({});
    const [loading, setLoading] = useState(false);
    const [modalMessage, setModalMessage] = useState("");
    const [isModalOpen, setIsModalOpen] = useState(false);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
        const { name, value, type, checked } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: type === "checkbox" ? checked : value,
        }));

        const updatedErrors = { ...errors };
        updatedErrors[name] = "";
        setErrors(updatedErrors);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const newErrors: Record<string, string> = {};

        if (!formData.firstName) {
            newErrors.firstName = "Kérlek, add meg a keresztnevedet.";
        }

        if (!formData.lastName) {
            newErrors.lastName = "Kérlek, add meg a vezetéknevedet.";
        }

        if (!formData.email) {
            newErrors.email = "Add meg az e-mail címedet.";
        }

        if (!formData.confirmEmail) {
            newErrors.confirmEmail = "Erősítsd meg az e-mail címedet.";
        }

        if (!formData.phoneNumber) {
            newErrors.phoneNumber = "Add meg a telefonszámodat.";
        }

        if (!formData.promoCode) {
            newErrors.promoCode = "Add meg a promóciós kódot.";
        }

        if (formData.email && !/\S+@\S+\.\S+/.test(formData.email)) {
            newErrors.email = "Az e-mail cím formátuma nem megfelelő.";
        }

        if (formData.email && formData.confirmEmail && formData.email !== formData.confirmEmail) {
            newErrors.confirmEmail = "A két e-mail cím nem egyezik.";
        }

        if (formData.phoneNumber && !/^\+?[0-9\s\-]{7,15}$/.test(formData.phoneNumber)) {
            newErrors.phoneNumber = "A megadott telefonszám formátuma nem érvényes.";
        }

        if (!formData.acceptedGameRules) {
            newErrors.acceptedGameRules = "A játékszabályzat elfogadása kötelező.";
        }

        if (!formData.acceptedPrivacyPolicy) {
            newErrors.acceptedPrivacyPolicy = "Az adatvédelmi szabályzat elfogadása kötelező.";
        }

        const hasErrors = JSON.stringify(newErrors) !== "{}";
        if (hasErrors) {
            setErrors(newErrors);
            return;
        }

        setLoading(true);
        try {
            const { confirmEmail, ...payload } = formData;
            const result = await submitPromoCode(payload);
            setModalMessage(result.message);

            if (result.success) {
                setFormData({
                    firstName: "",
                    lastName: "",
                    email: "",
                    confirmEmail: "",
                    phoneNumber: "",
                    promoCode: "",
                    acceptedPrivacyPolicy: false,
                    acceptedGameRules: false,
                });
            }
        } catch {
            setModalMessage("Hiba történt a beküldés során. Próbáld újra!");
        } finally {
            setLoading(false);
            setIsModalOpen(true);
        }

        console.log("request:", formData);

    };

    return (
        <div className="min-h-screen flex flex-col bg-[#1b2a3a] text-white">
            <div className="flex-grow flex justify-center items-center md:items-start md:pt-16 lg:pt-24">
                <form
                    onSubmit={handleSubmit}
                    noValidate
                    className="bg-[#22384d] w-full h-full p-6 md:p-10 md:rounded-xl md:shadow-2xl md:max-w-6xl text-white mx-auto"
                >
                    <h1 className="text-2xl md:text-4xl font-extrabold mb-2 text-center tracking-wide">KÓDFELTÖLTÉS</h1>
                    <p className="text-center text-gray-300 mb-6 md:mb-10 text-sm md:text-base">
                        Add meg az adataidat, és töltsd fel a csomagoláson található kódot!
                    </p>

                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 md:gap-6">
                        <FormInput
                            label="Vezetéknév*"
                            name="lastName"
                            value={formData.lastName}
                            onChange={handleChange}
                            error={errors.lastName}
                        />
                        <FormInput
                            label="Keresztnév*"
                            name="firstName"
                            value={formData.firstName}
                            onChange={handleChange}
                            error={errors.firstName}
                        />
                        <FormInput
                            label="Telefonszám*"
                            name="phoneNumber"
                            value={formData.phoneNumber}
                            onChange={handleChange}
                            error={errors.phoneNumber}
                        />
                        <FormInput
                            label="Email cím*"
                            name="email"
                            type="email"
                            value={formData.email}
                            onChange={handleChange}
                            error={errors.email}
                        />
                        <FormInput
                            label="Email cím megerősítése*"
                            name="confirmEmail"
                            type="email"
                            value={formData.confirmEmail}
                            onChange={handleChange}
                            error={errors.confirmEmail}
                        />
                        <FormInput
                            label="Promóciós kód*"
                            name="promoCode"
                            value={formData.promoCode}
                            onChange={handleChange}
                            error={errors.promoCode}
                        />
                    </div>

                    <div className="mt-6 space-y-3">
                        <div>
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
                            {errors.acceptedGameRules && (
                                <p className="text-red-400 text-sm mt-1">{errors.acceptedGameRules}</p>
                            )}
                        </div>

                        <div>
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
                            {errors.acceptedPrivacyPolicy && (
                                <p className="text-red-400 text-sm mt-1">{errors.acceptedPrivacyPolicy}</p>
                            )}
                        </div>
                    </div>

                    <p className="text-sm text-gray-400 mt-4">
                        Több kód feltöltésével növelheted nyerési esélyeidet.
                    </p>

                    <button
                        type="submit"
                        disabled={loading}
                        className={`w-full md:w-auto md:px-10 mt-6 py-3 rounded-md font-semibold transition mx-auto ${loading
                            ? "bg-gray-500 cursor-not-allowed"
                            : "bg-indigo-500 hover:bg-indigo-600"
                            }`}
                    >
                        {loading ? "Feltöltés..." : "Feltöltöm a kódot"}
                    </button>
                </form>
            </div>

            <Modal
                isOpen={isModalOpen}
                message={modalMessage}
                onClose={() => setIsModalOpen(false)}
            />
            <footer className="w-full bg-[#14212e] py-6 text-center md:text-right px-4 md:px-10 text-[14px] md:text-[15px] tracking-wide font-semibold text-gray-300 border-t border-[#1e2f41] flex flex-col md:flex-row md:justify-end md:items-center gap-3 md:gap-6">
                <a href="#" className="hover:text-white ml-6 transition-colors">Játékszabályzat</a>
                <a href="#" className="hover:text-white ml-6 transition-colors">Adatvédelmi szabályzat</a>
                <a href="#" className="hover:text-white ml-6 transition-colors">Cookie irányelvek</a>
                <a href="#" className="hover:text-white ml-6 transition-colors">Kapcsolat</a>
            </footer>

        </div >
    );
};

export default SubmissionForm;
