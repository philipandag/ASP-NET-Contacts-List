interface Category {
    id: number
    name: string
}

interface Contact {
    id: number
    name: string
    surname: string
    email: string
    phoneNumber: string
    mainCategory: Category
    subCategory: Category
    dateOfBirth: Date
}

function TSButton() {
    let name: string = "Fred";
    document.getElementById("TSButton").innerHTML = "Hello, " + name;
}

function Login() {
    let email: string = (<HTMLInputElement>document.getElementById("email")).value;
    let password: string = (<HTMLInputElement>document.getElementById("password")).value;


    let data = { Email: email, Password: password }

    var result = fetch("https://localhost:7020/api/Contacts/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
    .then(response => response.json())
    .then(data => {
        console.log('Login Success:', data);
    })
    .catch((error) => {
        console.error('Login Error:', error);
    })
}

async function getContacts(): Promise<Contact[]> {

    console.log("Getting contacts");
    const response = await fetch("https://localhost:7020/api/Contacts");
    const data = await response.json();
    return data;
}

function ContactOptions(id: number) {
    console.log("Options for contact " + id)
}

function LoadContacts() {
    console.log("Loading contacts")
    var list = document.getElementById("ContactsList");

    getContacts().then(contacts => {
        contacts.forEach(contact => {
            console.log(contact);
            var item = document.createElement("li");

            item.className = "Contact-Item";

            var name = document.createElement("span");
            name.className = "Contact-Name";
            name.innerHTML = contact.name;
            item.appendChild(name);

            var surname = document.createElement("span");
            surname.className = "Contact-Surname";
            surname.innerHTML = contact.surname;
            item.appendChild(surname);

            var mainCategory = document.createElement("span");
            mainCategory.className = "Contact-MainCategory";
            mainCategory.innerHTML = contact.mainCategory.name;
            item.appendChild(mainCategory);

            var subCategory = document.createElement("span");
            mainCategory.className = "Contact-Subcategory";
            mainCategory.innerHTML = contact.subCategory.name;
            item.appendChild(subCategory);

            var optionsButton = document.createElement("button");
            optionsButton.className = "btn btn-primary btn-Contact-Options";
            optionsButton.innerHTML = "...";
            optionsButton.onclick = () => ContactOptions(contact.id);
            item.appendChild(optionsButton);


            list.appendChild(item);
        })
    })

}

window.onload = LoadContacts;
