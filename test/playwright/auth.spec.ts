import { test, expect, Locator } from '@playwright/test';
import * as users from './test-data/users.json'

test('is alive', async ({ page }) => {
  await page.goto('/');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle("Case Assessment and Tracking System (CATS)");
});
 
test('login unsuccessfully with invalid credentials', async ({ page }) => {
	await page.goto('/');

	let preLoginUrl = page.url();
  
	// Expects page to have a heading with the name of Sign In.
	await expect(page.getByRole('heading', { name: 'Sign In' })).toBeVisible();
  
	let form = page.locator('form');

	await login(form, { 
	  username: "john.doe@example.com", 
	  password: "IncorrectPassword12345!"
	});

	// Expects "Invalid login attempt" alert
	await expect(form.getByRole('alert')).toContainText("Invalid login attempt");
	
	// Expects unsuccessful navigation, remaining on the login page
	await expect(page).toHaveURL(preLoginUrl);
});

test('login successfully with valid credentials', async ({ page }) => {
  await page.goto('/');

  // Expects page to have a heading with the name of Sign In.
  await expect(page.getByRole('heading', { name: 'Sign In' })).toBeVisible();
  
  let form = page.locator('form');
  
  await login(form, { 
	username: users.regular.username, 
    password: users.regular.password
  });

  // Expect dashboard to be visible
  await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible();

  // Expect successful navigation, navigating away from login page
  await expect(page).toHaveURL('/');
});

async function login(form: Locator, user: User) {
    await form.locator('input[type="text"]').fill(user.username);
    await form.locator('input[type="password"]').fill(user.password);
    await form.locator('button[type="submit"]').click();
}

interface User {
    username: string;
    password: string;
}