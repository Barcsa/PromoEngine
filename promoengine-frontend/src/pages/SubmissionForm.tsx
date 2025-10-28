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
        <div className="relative z-0 min-h-screen flex flex-col text-white bg-gradient-to-b from-[#1c3b52] to-[#2d5b78] md:from-[#0e2a3b] md:to-[#1b4054] overflow-hidden">
            <div className="h-8 bg-[#0b2230] border-b-4 border-[#4da0db]" />

            <svg className="absolute top-0 left-0 w-full" viewBox="0 0 1440 280">
                <path
                    fill="#0e2a3b"
                    d="M0,192L80,213.3C160,235,320,277,480,272C640,267,800,213,960,186.7C1120,160,1280,160,1360,160L1440,160L1440,0L0,0Z"
                ></path>
            </svg>

            <div className="flex-grow flex justify-center items-center md:items-start md:pt-16 lg:pt-24 px-4 md:px-10 z-10">
                <form
                    onSubmit={handleSubmit}
                    noValidate
                    className="relative z-10 w-full text-white mx-auto p-4 md:p-10 bg-transparent md:bg-[#1f3b52]/90 md:backdrop-blur-sm md:rounded-3xl md:border md:border-[#3d556a] md:max-w-6xl"
                >
                    <h1 className="text-2xl md:text-5xl font-extrabold mb-2 text-center tracking-wide text-[#b8dcf5]">
                        KÓDFELTÖLTÉS
                    </h1>
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
                            autoComplete="new-email"
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
                                <p className="text-red-400 text-sm mt-1">
                                    {errors.acceptedGameRules}
                                </p>
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
                                <p className="text-red-400 text-sm mt-1">
                                    {errors.acceptedPrivacyPolicy}
                                </p>
                            )}
                        </div>
                    </div>

                    <p className="text-sm text-gray-400 mt-4">
                        Több kód feltöltésével növelheted nyerési esélyeidet.
                    </p>

                    <div className="flex justify-center mt-8">
                        <button
                            type="submit"
                            disabled={loading}
                            className={`w-full md:w-auto md:px-10 py-3 rounded-full font-semibold transition shadow-md ${loading
                                ? "bg-gray-500 cursor-not-allowed"
                                : "bg-[#4da0db] hover:bg-[#63b4eb]"
                                } text-[#0e2a3b]`}
                        >
                            {loading ? "Feltöltés..." : "FELTÖLTÖM A KÓDOT"}
                        </button>
                    </div>
                </form>
            </div>

            <svg
                className="hidden md:block absolute bottom-32 right-0 w-full md:w-[90%] opacity-50 pointer-events-none z-0"
                viewBox="0 0 1200 200"
            >
                <path
                    d="M1440,240 C1150,180 900,120 600,180 C300,240 150,140 0,160"
                    fill="none"
                    stroke="#b8dcf5"
                    strokeWidth="2"
                    strokeDasharray="6 10"
                    strokeLinecap="round"
                >

                </path>
            </svg>


            <img
                src="/airplane-svgrepo-com.svg"
                alt=""
                className="hidden md:block absolute top-[690px] left-[90px] md:bottom-[90px] md:right-[280px] w-[90px] md:w-[120px] opacity-80 rotate-[10deg] z-[0]"
            />

            <svg className="absolute bottom-0 left-0 w-full">
                <path
                    fill="#4da0db"
                    d="M0,288L80,288C160,288,320,288,480,272C640,256,800,224,960,224C1120,224,1280,256,1360,272L1440,288L1440,320L0,320Z"
                ></path>
            </svg>


            <footer className="w-full bg-[#0f2b3a] py-6 text-[#c8e3f6] text-center font-semibold z-10 flex flex-col md:flex-row md:justify-center gap-3 md:gap-6">
                <a href="#" className="hover:underline">
                    JÁTÉKSZABÁLYZAT
                </a>
                <a href="#" className="hover:underline">
                    ADATVÉDELMI SZABÁLYZAT
                </a>
                <a href="#" className="hover:underline">
                    COOKIE IRÁNYELVEK
                </a>
                <a href="#" className="hover:underline">
                    KAPCSOLAT
                </a>
            </footer>

            <Modal
                isOpen={isModalOpen}
                message={modalMessage}
                onClose={() => setIsModalOpen(false)}
            />
        </div>
    );
};

export default SubmissionForm;
