import React from "react";
import Banking from "./Banking";


export default function Ebanking() {
	return (
		<Banking>
			<Banking.Charge />
			<Banking.Deposit />
			<Banking.Wallet />
			<Banking.Credit />
		</Banking>
	);
}
